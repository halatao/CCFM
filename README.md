# CiscoConfigurationFileManager

CiscoConfigurationFileManager allows users to create, modify, and display configurationd for Cisco network devices. It's a user-friendly networking tool that provides guidance on theoretical knowledge related to the configuration file. This makes it suitable for users without prior knowledge of Cisco IOS and for educational purposes.

## Table of Contents

- [Getting Started](#getting-started)
- [Basic Usage](#basic-usage)
  - [Connecting to device](#connecting-to-device)
    - [Serial connection](#serial-connection)
    - [SSH connection](#ssh-connection)
  - [Configuration of device](#configuration-of-device)
    - [Serial Connection](#serial-configuration)
    - [SSH Connection and No Connection](#seria-and-no-connection-configuration) 
        - [Parts of View](#parts-of-view) 
        - [Icon Bar](#icon-bar)
        - [Line for configuration](#line-for-configuration)
        - [Operations with Entire Configuration](#opearation-with-entire-configuration)
          - [Upload Configuration](#upload-configuration)
          - [Save Configuration as File](#save-configuration-as-file)
          - [Open Confguration from File](#open-configuration-from-file)
          - [New Configuration](#new-configuration)
- [Advanced Usage](#advanced-usage)
- [Contributing](#contributing)
- [License](#license)

## Getting Started
To use this application, follow these simple steps:

1. Download the latest release of the application from the "Release" section of this repository.
2. Extract the downloaded file to a location of your choice on your computer.
3. Open the extracted folder and double-click the application executable.
4. If your computer's firewall prompts you to allow the application to access the internet, select "Allow access".

That's it! You should now be able to use the application without any issues. If you have any questions or run into any problems, please consult the documentation or open an issue on this repository.

## Basic Usage
When you start application you can see on default page three modes in which you can modify configuration.

![image](https://user-images.githubusercontent.com/99823777/233916354-1c615617-d97a-44e1-a134-1816d6c1b990.png)
### Connecting to device
#### Serial Connection
To connect through serial you need to have network device connected through COM port and then select it and connect.

![image](https://user-images.githubusercontent.com/99823777/233918230-8aff01c5-83bc-4c7c-a627-db7ac336d3ab.png)

#### SSH Connection
 First you have to enter SSH connection parameters to be able to communicate with device.

![image](https://user-images.githubusercontent.com/99823777/233929357-de629a76-b100-4b5c-b2d6-b0a0d8876b81.png)

### Configuration of Device
#### Serial Configuration
Serial connections offers you configurations of functions which cannot be configured through SSH. To configure a configuration module to device you have to enter a credentials and optionaly other parameters and apply selected configuration.

![image](https://user-images.githubusercontent.com/99823777/233919950-0ca99476-5c50-49a3-97ac-56661d336dfb.png)

#### SSH and No Connection Configuration
##### Parts of View
SSH connection a no connection to network device uses same view. You are able to view, modify and save configuration in this view.
View is divided into three main parts

- List of Modules in Configuration

List contains names of modules which are currently in configuration. Each module contains lines of commands sorted by their category.

- Lines of Selected Module

Selected module shows which lines are currently in selected module. User can add new line, modify existing ones or delete a line.

- Lines of Example Module

User can optinaly show how could configuration look like by click on a question mark icon on icon bar at top.

![image](https://user-images.githubusercontent.com/99823777/233931914-35a99ff0-63c6-4d01-a08d-8f4c5a8168fd.png)

##### Icon Bar
Icon bar contains key funcionaly of the application.

![image](https://user-images.githubusercontent.com/99823777/233935888-87d24217-e160-4c9b-aa71-c23cfeb6dc0f.png)

- ![image](https://user-images.githubusercontent.com/99823777/233936203-0a997461-22f1-46dd-a873-ed477be02e91.png) Navigate at home page (type of connection page).
- ![image](https://user-images.githubusercontent.com/99823777/233936415-70c3841c-ef07-4bee-b6a4-a15aa27d9a29.png) - Navigate one step back.
- ![image](https://user-images.githubusercontent.com/99823777/233936558-87c95496-e6fa-4710-a587-5d9f6673908d.png) - Saves changes in currently selected module in modules.
- ![image](https://user-images.githubusercontent.com/99823777/233936669-fe1a7007-fe23-44eb-9487-ba2ee2d5ef49.png) - Saves changes of all modules.
- ![image](https://user-images.githubusercontent.com/99823777/233937032-a4ebe4dc-b30a-4118-a92c-74a4cd95b217.png) - Add empty line to currently selected module.
- ![image](https://user-images.githubusercontent.com/99823777/233937186-34ec1d6e-6db8-4ac8-88a6-7b1d69376ef6.png) - Delete currently selected module.
- ![image](https://user-images.githubusercontent.com/99823777/233937374-69612b6f-abc9-4b8d-8d6f-035b74e93252.png) - Toggle example module configuration.
- ![image](https://user-images.githubusercontent.com/99823777/233939172-fdb8f9e4-3266-4cb2-b89c-7e5aea2f39d2.png) - Add selected example module to configuration

##### Line for Configuration
In this line you can enter, edit or delete configuration commands. If you want to delete entire line, you have to click on the garbage icon on the right.
![image](https://user-images.githubusercontent.com/99823777/233974271-71137666-f357-4319-93fa-a72bb74a15b5.png)

Entered commands are partly validated. Application validate if the configuration commands are in correct format. If you like is unsupported doesn't mean the command is wrong. Support of commands is limited. You can expand your command support, by steps in [advanced usage](#advanced-usage).
![image](https://user-images.githubusercontent.com/99823777/233974441-1521a3da-a2ba-494b-9218-0db24a9c9b32.png)

##### Operations with Entire Configuration
##### Upload Configuration
You can upload configuration by selecting:
**Configuration** > **Merge configuration**

#### Save Configuration as File
You can save configration to your computer by selecting:
**File** > **Save As**

##### Open Configuration from File
You can open existing configuration from you compouter by selecting:
**File** > **Open**

##### New Configuration
You can use default configuration if you currently do not have one by selecting:
**File** > **New**

## Advanced Usage
### Modifications
Note that changing those configuration can affect performance of application.
### Regex modification
You can modify command support by adding new regex for validation of entered commands by adding new regexs in configuration file.

**Help** > **Show regex configuration**

### Category modification
You can modify categories and category sorting by modifying dictionary of category names and key words from which they are sorted  in configuration file.

**Help** > **Show category configuration**

## Contributing

We welcome contributions to this project! Here's how you can get started:

Fork this repository to your own account.
1. Clone the forked repository to your local machine.
2. Create a new branch for your changes.
3. Make your changes, and commit them with descriptive commit messages.
4. Push your changes to your forked repository.
5. Submit a pull request to the main repository.
6. Before submitting your pull request, please make sure that:

- Your code adheres to our coding standards.
- Your changes are well-tested and documented.
- Your pull request includes a clear and detailed description of the problem you're solving and the solution you're proposing.
- By contributing to this project, you agree to abide by our code of conduct.

If you have any questions or need help with contributing, please open an issue in the repository or contact ondrej.halata@gmail.com directly.

## License

This project is licensed under the MIT licence. 

The following packages are used in this project and are subject to their own licenses:

- CommunityToolkit.Mvvm: MIT License
- Extended.Wpf.Toolkit: Microsoft Public License (Ms-PL)
- MaterialDesignThemes: MIT License
- Microsoft.Extensions.Configuration.Json: Apache License 2.0
- Microsoft.Extensions.DependencyInjection: Apache License 2.0
- Microsoft.Xaml.Behaviors.Wpf: Microsoft Public License (Ms-PL)
- MicrosoftExpressionInteractions: Microsoft Public License (Ms-PL)
- ReactiveProperty: MIT License
- ShowMeTheXAML.MSBuild: MIT License
- SSH.NET: MIT License
- System.IO.Ports: MIT License
- Tftp.Net: MIT License

Note that these licenses may be subject to change, so it's a good idea to double-check the license information for each package before including it in your project.
