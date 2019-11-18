# DotNet-Deployer
Deploy your .NET Project to Raspberry Pi / Linux x64 with ease!
This will help you deploy your NET Core applications (Console or ASP.NET) to Raspbian or other Linux distro in the blink of an eye. 

# Requirements

- Linux device: Currently supported Raspberry Pi (Linux ARM) and generic Linux x64 distros.
- [.NET Core 3.0](https://www.microsoft.com/net/download)
- SSH enabled on your target machine (Raspberry / Linux)

# Installation
- Open a command prompt
- Run `dotnet tool install --global dotnet-ssh --version 2.0.0`
- You're ready to go!

# How to deploy your application?

## 1. Create a profile

Currently, you need to create a deployment profile with all the data required for the deployments.

1.  Go to the root directory of the project you want to deploy
    1. Run the appropriate command    
        1. For a Raspberry Pi: `dotnet-ssh create --name MyRaspberry --target-device Raspbian`
        2. For a generic Linux x64 machine: `dotnet-ssh create --name Ubuntu --target-device GenericLinux64`
    
		This will create a template inside the folder named **shh-deployment.json**
2. Edit this file and fill it with your parameters. You will want to set the host, the credentials, username, destination path...

	Take this screeenshot as reference. It contains 2 different profiles, one for Raspberry Pi and another for my Ubuntu machine.

![Ssh Deployment Json](Docs/Ssh-Deployment-Json.png)

3. After you've finished, save the file.

## 2. Deploy your application

After you have configured **ssh-deployment.json** you just need to run the tool using this command
```
dotnet-ssh deploy --name MyRaspberry 
```

Please, notice that the `--name` argument provides the profile that you want to use. It will take all the information in it to perform the deployment

# Running your application in the target device
You should install .NET Core into your target device! 

## Installing .NET Core 2.1 for Raspbian

Execute the following commands inside a terminal
```
sudo apt-get -y update
sudo apt-get -y install libunwind8 gettext
wget https://dotnetcli.blob.core.windows.net/dotnet/Sdk/2.1.300/dotnet-sdk-2.1.300-linux-arm.tar.gz
wget https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/2.1.0/aspnetcore-runtime-2.1.0-linux-arm.tar.gz
sudo mkdir /opt/dotnet
sudo tar -xvf dotnet-sdk-2.1.300-linux-arm.tar.gz -C /opt/dotnet/
sudo tar -xvf aspnetcore-runtime-2.1.0-linux-arm.tar.gz -C /opt/dotnet/
sudo ln -s /opt/dotnet/dotnet /usr/local/bin
dotnet --info
```
3. Go to the directory where you deployed you application
4. Run it by using `./[NameOfYourApp]`, for example `./HelloWorld`. If you don't know the name, just type `ls` and look for the file with no extension in a different color (non-executable files are usually in light gray text)

Easy enough? Try it and freak it out!

## Installing .NET Core in other Linux distros

Refer to https://dotnet.microsoft.com/ for download and install instructions 🐔
