# Lim

Lim is a custom programming language I created. It started as a passion project around 2022 and continues to be developed in my free time alongside my studies and work.

This branch represents a new version of the codebase. It employs the "visitor" design pattern, along with a new tree structure called "High Intermediate Representation" (HIR), which is used for symbol and generic resolution. The goal is to decouple the code, making it cleaner and easier to extend.

<p align="center">
    <img src="https://github.com/ruffaultravenelg/limc/blob/visitors/lim-doc/static/img/logo.png?raw=true" alt="Lim's logo" width="150px">
</p>

> [!NOTE]
> Lim is a non-professional project built primarily for fun, learning, and experimentation.

What began as a quest to build my "dream language" evolved as my skills grew. I realized that language design goes far beyond syntax alone. Today, the goal is to build a complete and usable version of Lim.

---

## Syntax & Philosophy

Lim takes inspiration from the simplicity and minimal feel of **C**, but enhances it with convenient modern features:

- Lightweight [garbage collector](https://github.com/orangeduck/tgc)
- Generic types
- Simple classes
- Tagged unions (enums holding associated data)

---

## How It Works

Lim uses a **transpilation** approach by first compiling to C, then using an external compiler to create the executable.

The main pipeline is the following:
1. Convert source files into tokens (lexing)
2. Parse tokens into an Abstract Syntax Tree (AST)
3. Convert the AST into a HIR, resolving generic types and replacing string identifiers with symbol references
4. Compile the HIR into a C source file
5. Use an external C compiler (such as GCC) to compile the C code into a native executable

---

## Main directories

| Folder | Description |
| --- | --- |
| `/limc` | Main Visual Studio solution (.slnx) |
| `/limc/limc` | The Lim compiler source code (written in VB.NET) |
| `/limc/limc.tests` | Functional and non-regression tests |
| `/limc/limc/runtime` | Lim's C runtime |
| `/lim-samples` | Lim code samples |
| `/lim-docs` | Language documentation built with [Docusaurus](https://docusaurus.io/) |
| `/lim-website` | The Lim project website source |
| `/lim-syntax-highlighting` | Text editor extensions for Lim syntax highlighting |

---

## Getting Started

> [!WARNING]
> Lim is currently under active development. There is not yet a stable release branch.

If you would like to test the current build:

1. **Prerequisites:** Ensure you have `gcc` (or another compatible C compiler) installed and available in your system path.
2. **Build the compiler:** Open the `/limc` solution in **Visual Studio** and build the release executable for your operating system.
3. **Permissions:** Ensure the newly built executable has execution permissions.
4. **Run:** Open your terminal and run `limc --help` to view CLI options and usage.