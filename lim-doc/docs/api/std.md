---
slug: std
sidebar_position: 1
description: std
---

# Standard library
The `std` library contains all the core types and functions of the Lim language. It is automatically included in all source files, so there is no need to import it manually.
| Symbol | Item Type |
|---|---|
| 𝒇 | Function |
| 𝒮 | Struct |
| 𝒞 | Class |
| 𝓂 | Method |
| 𝓟 | Property |


## 𝒇 : puts
The `puts` function, short for "put string", writes a string followed by a newline to standard output. It is useful for displaying text in the console.

#### Syntax
```c
puts(message:str)
```

- `message`
    - **Type**: [`str`](https://www.google.com/search?q=%23str)
    - The message to write to the standard output.



*Does not return a value.*

#### Examples

```c
// Simple example
puts('Hello world')

// Example with a variable
let name = 'Alice'
puts('Hello ' + name)

// Multi-line string example
puts('Line 1\nLine 2\nLine 3')

```

---

## 𝒇 : puti

The `puti` function, short for "put integer", writes an integer on a new line to the standard output. It is useful for displaying a number in the console.

#### Syntax

```c
puti(value:int)

```

- `value`
    - **Type**: [`int`](https://www.google.com/search?q=%23int)
    - The number to write to the standard output.



*Does not return a value.*

#### Examples

```c
puti(3)
puti(6 + 53)

```

:::note
Lim does not automatically convert between different numeric types. Therefore, this function cannot accept floating-point numbers.
:::

---

## 𝒇 : gets

The `gets` function, short for "get string", reads a line of text from the standard input. It is useful for retrieving user input as a string.

#### Syntax

```c
gets():str
gets(message:str):str

```

- `message`
    - **Type**: [`str`](https://www.google.com/search?q=%23str)
    - The message to display in the console before prompting for a value.



*Returns a value of type [`str`](https://www.google.com/search?q=%23str).*

#### Examples

```c
// Simple example with a prompt message
let input = gets('Please enter a digit:')
puts('You entered: ' + input)

// Without a prompt message
puts('You entered: ' + gets())

```

---

## 𝒇 : geti

The `geti` function, short for "get integer", reads an integer from the standard input. It is useful for retrieving user input as an integer.

#### Syntax

```c
geti():int
geti(message:str):int

```

- `message`
    - **Type**: [`str`](https://www.google.com/search?q=%23str)
    - The message to display in the console before prompting for a value.



*Returns a value of type [`int`](https://www.google.com/search?q=%23int).*

#### Examples

```c
// Simple example
let number = geti()
puti(number * 2)

```

:::caution
If the user input is not a valid integer, an error will be raised.
:::

---

## 𝒮 : int

The `int` struct represents a 32-bit signed integer in memory.

:::caution
This struct does not have a constructor; every number in the code is an instance of this struct. Therefore, `5` is of type `int`.
:::

### 𝓂 : str

Returns the string representation of the number.

```js
let age = 19
let message = 'You are ' + age.str() + ' years old'

```

### 𝓂 : float

Returns the floating-point representation of the integer. `5` becomes `5.0`.