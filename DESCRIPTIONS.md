# 📁 Project File Overview

Below is a summary of the key files and their roles within the **PebbleCode** application.

# 🧩 **App.xaml & App.xaml.cs**

These files define the **entry point** and **global structure** of the application.

* **`App.xaml`**
  Configures application-wide resources, theming, and styling.

* **`App.xaml.cs`**
  Handles application lifecycle events and launches the main window.

# 🪟 **MainWindow\.xaml & MainWindow\.xaml.cs**

These files define the **main user interface** and **interaction logic** for the PebbleCode IDE.

* **`MainWindow.xaml`**
  Provides the visual layout of the app, including:

  * Code editor
  * Output console
  * Control buttons (Run, Save, etc.)

* **`MainWindow.xaml.cs`**
  Implements the event handlers for:

  * Running code
  * Saving files
  * Updating the output console

# 🧠 **PebbleCodeInterpreter.cs**

Contains the **core interpreter logic** for the PebbleCode language.

* Parses and executes user-written code line by line.
* Currently supports basic commands like `say` to display output.
* Designed to be extensible, allowing new syntax features to be added easily.

---

### 💾 **FileSaver.cs**

Handles the **file saving functionality** using the Windows Storage API.

* Presents a file save dialog using `FileSavePicker`.
* Writes the current contents of the code editor to a `.txt` file.
* Integrates with WinUI 3 using window handle interop for seamless UX.

# 🧱 **PubbleCode.csproj**

The **project configuration file** that defines how the app is built.

* **Target Framework**: `.NET 8.0`
* **Platform Target**: `windows10.0.19041.0`
* **Project Type**: WinUI 3 desktop application

This file also manages NuGet dependencies and SDK references.

# 📜 **app.manifest**

The application’s **manifest file**, used to define system level behavior.

* Can be extended to:

  * Associate specific file types
  * Request elevated permissions
  * Enable DPI awareness for high resolution displays


