# C Interoperability

Lim provides direct interaction with the C ecosystem. Thanks to preprocessor directives and a dedicated syntax, you can embed native C code, include external libraries, and manipulate the compiler's internal mechanisms.

:::info
Generally, the `$` symbol indicates direct interaction with the generated C code.
:::

## Preprocessor Directives

### Header Inclusion (`$include`)

The `$include` directive inserts an `#include` clause directly at the top of the generated C file. It is the essential tool for accessing the standard library (`stdio.h`, `math.h`, etc.) or third-party libraries.

```go
$include "<math.h>"
$include "my_custom_lib.h"

```

### Compiler Options (`$comopt`)

The `$comopt` (*Compiler Option*) directive passes specific arguments to the target compiler (`gcc` or `clang`). This is particularly useful for linking.

```go
// Example: Linking with the math library
$comopt "-lm"

```

## Native C Code Insertion

You can inject C code directly inside your functions. Any line starting with the `$` symbol is treated as raw C source code and inserted as-is at the corresponding location in the generated file.

```go
func main
    $ for (int i = 0; i < 5; i++) {
    $     printf("[%d] Hello from C!\n", i);
    $ }

```

:::warning Warning
The `limc` compiler does not validate C syntax inside `$` lines. Make sure not to omit semicolons (`;`) and to adhere to C language syntax.
:::

## Accessing Lim Elements from C

One of the key strengths of `limc` is the ability to reference Lim variables directly within your C code blocks.

### Variables (`$variableName`)

The `limc` compiler often renames variables to avoid collisions or manage scoping. To access the actual name of a variable in the generated C code, use the `$variableName` syntax.

**Example in Lim:**

```go
func main
    let maVariable = 42
    $ printf("Value: \%d\n", $maVariable);

```

**Compiled Result (Conceptual):**

```c
void main_f(Ctx_t _c) {
    Ctx_t c = { _c, "main", _c.gc, NULL };
    int v_0 = 42; // 'maVariable' was renamed to 'v_0'
    printf("Value: %d\n", v_0); // $maVariable is properly replaced
}

```

### Internal Constants (`@CONSTANT_NAME`)

`limc` uses a set of constants to structure the generated code (context management, allocation, etc.). You can access them via the `@CONSTANT_NAME` syntax.

| Name | Description | Value |
| --- | --- | --- |
| `@RUNTIME_CONTEXT_STRUCT_NAME` | Name of the struct containing a function call context | `CT_t` |
| `@RUNTIME_CONTEXT_VARIABLE_NAME` | Name of the variable containing the context inside a function | `c` |
| `@PANIC_FUNCTION_NAME` | Name of the panic (crash) function | `lim_panic` |
| `@PRINT_STACK_TRACE_FUNCTION_NAME` | Name of the function used to print the stack trace | `lim_printStackTrace` |
| `@LIM_ALLOC` | Name of the dynamic memory allocation function (garbage-collected) | `LIM_ALLOC` |
| `@LIM_FREE` | Name of the memory deallocation function (the freed block must have been allocated with `LIM_ALLOC`) | `LIM_FREE` |
| `@LIM_REALLOC` | Dynamic reallocation function (garbage-collected) | `LIM_REALLOC` |
| `@INT_TO_STR_BUFFERSIZE` | Buffer size for integer-to-string conversions | `12` |
| `@FLOAT_TO_STR_BUFFERSIZE` | Buffer size for float-to-string conversions | `12` |
| `@GETS_BUFFER_SIZE` | Buffer size for `gets()` input | `100` |
| `@INSTANCE_ARGUMENT_NAME` | Name of the instance parameter for methods | `instance` |
| `@SELF` | Alias for `INSTANCE_ARGUMENT_NAME` | `instance` |

## Native Attributes (C-Fields)

Inside a class or struct, you can define fields that exist solely in C. These fields are added to the resulting C struct, but are ignored by Lim's type system (they can only be accessed via `$` code lines).

```go
class FileReader
    $ FILE *handle; // Purely C attribute

    new(filepath: str)
        $ $handle = fopen($filepath, "r");

```

## Memory Allocation

There are two ways to dynamically allocate memory in Lim:

1. **Manual allocation:** Using standard `malloc()` functions. You are responsible for freeing the memory with a matching `free()` (class destructors are well-suited for this).
2. **Garbage-collected allocation:** Letting the garbage collector manage memory using Lim's built-in allocation functions.

Naturally, explicitly managing an object's lifecycle whenever it is predictable remains the best practice.

**Allocation in C:**

```c
my_struct* data = @LIM_ALLOC(sizeof(my_struct));
...
@LIM_FREE(data);

```

**Managing a class:**

```lim
class my_exemple
    $ thing_t* data;

    new()
        $ "@SELF->data = @LIM_ALLOC(sizeof(thing_t));"
    
    end()
        $ "@LIM_FREE(@SELF->data);"

```

## String Management

Lim does not use `char*` for strings. Instead, it relies on a struct named `@LIM_STR`.

```c
typedef struct {
    char* data;
    size_t len;
} lim_str;

```

You can convert between string representations using the following functions:

* `char* @LIM_STR_TO_C_STR(@LIM_STR in)`: returns the underlying pointer contained in the Lim string.
* `@LIM_STR @C_STR_TO_LIM_STR(char* in)`: creates a Lim string using the provided pointer (does not duplicate the string).
