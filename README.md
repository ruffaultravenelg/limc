# Lim

Lim is a custom programming language created by Gémino RUFFAULT--RAVENEL. It started as a passion project around 2022 and continues to be developed in my free time alongside my studies and work.

> [!NOTE]
> Lim is a non-professional project built primarily for fun, learning, and experimentation.

What began as a quest to build my "dream language" evolved as my skills grew. I realized that language design goes far beyond syntax only. Today, the goal is to build a complete and usable version of Lim.

---

## Syntax & Philosophy

Lim takes inspiration from the simplicity and minimal feel of **C**, but enhances it with convenient modern features:

- Includes a [lightweight garbage collector](https://github.com/orangeduck/tgc).
- Generic Types
- Simple classes
- Tagged unions (enums holding associated data)

---

## How It Works

Lim uses a **transpilation** approach:

1. The Lim compiler converts Lim source code into standard C.
2. An external C compiler (such as GCC) compiles the C code into a native executable.

---

## Repository Structure

| Folder | Description |
| --- | --- |
| `/limc` | The Lim compiler source code (written in VB.NET) |
| `/lim-docs` | Language documentation built with [Docusaurus](https://docusaurus.io/) |
| `/lim-core` | Houses both the standard library and the native runtime environment. |
| `/lim-core/libs` | Standard libraries written in Lim (e.g., `std.lim`, `math.lim`) |
| `/lim-core/clibs` | C implementations for low-level language support (e.g., garbage collector) |
| `/lim-website` | The Lim project website source |
| `/lim-syntax-highlighting` | Text editor extensions for Lim syntax highlighting |

---

## Getting Started

> [!WARNING]
> Lim is currently under active development. There is not yet a stable release branch.

If you would like to test the current build:

1. **Build the compiler:** Open the `/limc` solution in **Visual Studio** and build the release executable for your operating system.
2. **Setup libraries:** Copy the content of `/lim-core` directory into the same folder as the compiled executable.
3. **Prerequisites:** Ensure you have `gcc` (or another compatible C compiler) installed and available in your system path.
4. **Run:** Open your terminal and run `lim --help` to view CLI options and usage.