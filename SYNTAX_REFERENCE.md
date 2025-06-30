# 📘 Full Syntax Reference (PuddleCode)

> ✅ *All tutorials will be fully demonstrated inside the PebbleCode app.*



# 📤 1. Output

```
say "Hello, world!"
say "The answer is " + answer
```

* Prints text or variable content to the output console.
* Supports string and variable concatenation using `+`.



# 📦 2. Variables

```
set name to "Juntheng"
set age to 12
set total to age + 3
```

* Declares or updates a variable.
* Types are automatically inferred (string or number).



# 🔁 3. Loops

```
repeat 5 times
    say "I love code!"
end
```

* Repeats the enclosed block a fixed number of times.
* Supports nesting with other statements.



# ❓ 4. Conditionals

### Basic `if`

```
if age > 10 then
    say "You're older than 10"
end
```

### `if ... otherwise`

```
if score == 100 then
    say "Perfect!"
otherwise
    say "Keep trying!"
end
```

* Allows basic logic and decision-making.
* Every `if` must use `then` and end with `end`.



# ➕ 5. Expressions & Operators

```
set x to 5 + 3 * 2
set isCorrect to 4 == 2 + 2
```

### Supported Operators:

| Type       | Operators                        |
| ---------- | -------------------------------- |
| Arithmetic | `+`, `-`, `*`, `/`, `%`          |
| Comparison | `==`, `!=`, `>`, `<`, `>=`, `<=` |



# 💬 6. Comments

```
# This is a comment
```

* Use comments to explain code or leave notes.
* Whole-line only; no inline comments supported.



# 🧠 7. Functions

```
define greet into "John"
    say "Welcome!"
end

greet "John"
```

* Define reusable code blocks.
* Parameters (e.g., `define greet with name`) are supported in advanced mode.

---

# 📥 8. Input

```
ask "What is your name?" into userName
say "Hello, " + userName
```

* Prompts the user and stores the input into a variable.



## 🔂 9. Nested Blocks

```
repeat 3 times
    if score > 5 then
        say "Nice!"
    end
end
```

* All control structures (`repeat`, `if`, `define`, etc.) must be closed with `end`.



# 🧱 Structural Rules

* Each command must start on a **new line**
* **Indentation is optional** (but encouraged for readability)
* Strings must be enclosed in **double quotes**
* Statements must be **complete**
  *( example : `set x to 5`, not just `set x`)*

---

Would you like this exported as a PDF or integrated as an in-app tutorial tab? I can also help design a collapsible sidebar version for WinUI 3.

