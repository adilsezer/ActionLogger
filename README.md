
# ActionLogger

**ActionLogger** is a Windows desktop app that monitors and logs user-initiated activities such as application launches, file access, mouse clicks, and keyboard inputs. It’s ideal for tracking productivity and refining RPA project details.

![Alt text](https://github.com/adilsezer/ActionLogger/blob/master/Assests/actionlogger_screenshot.png?raw=true "ActionLogger App")
![ActionLogger Screenshot](assets/actionlogger_screenshot.png)

## Features

- **Application Launch Tracking**: Logs user-initiated application starts.
- **File Access Monitoring**: Logs access to files in a monitored directory.
- **Mouse Click Monitoring**: Logs which application window the mouse was clicked in, including the button used (left or right click) and the active window title.
- **Keyboard Input Logging**: Logs typed words and key presses.
- **Clipboard Change Detection**: Monitors and logs changes to the clipboard.
- **Real-Time Filtering**: Search through logs easily.
- **User-Friendly UI**: Built with WPF for a clean interface.

## Installation

1. **Prerequisites**: Windows 10+, .NET Framework 4.7.2+.
2. **Download**: Clone or download the latest release from GitHub.
3. **Build**: Open in Visual Studio, restore packages, and build the solution.
4. **Run**: Execute the app for full functionality.

## Usage

- **Monitor Actions**: Track user actions, including app starts, file access, mouse clicks, keyboard inputs, and clipboard changes.
- **File Access Monitoring**: Set up monitoring of specific directories (e.g., Documents) to log file access events.
- **Filter Logs**: Use the filter to quickly find specific actions.
- **Typing Buffer**: The app intelligently groups typed characters into words and logs them with proper formatting.
- **Stop Logging**: Close the app to stop logging.

## Troubleshooting

- **No Logs**: Verify necessary permissions are present.
- **Crashes**: Check for missing dependencies or permission issues.

## License

Licensed under the [MIT License](https://github.com/adilsezer/ActionLogger/blob/master/LICENSE.txt).
