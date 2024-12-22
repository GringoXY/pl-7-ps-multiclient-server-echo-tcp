# pl-7-ps-multiclient-server-echo-tcp
Project of the multi-client ECHO TCP server-client for the faculty "PS" (Network Programming) in Lodz University of Technology

# Docs
Refer to [PS - Lab 01 - EchoKlientTCP.pdf](./docs/PS%20-%20Lab%2001%20-%20EchoKlientTCP.pdf)

Refer to [PS - Lab 02 - EchoSerwerTCP](./docs/PS%20-%20Lab%2002%20-%20EchoSerwerTCP.pdf)

Refer to [PS - Lab 04 - EchoSerwerTCPWielowatkowy](./docs/PS%20-%20Lab%2004%20-%20EchoSerwerTCPWielowatkowy.pdf)

## Requirements
Install [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Run program via CLI
Make sure you conform [Requirements](#requirements) and your CLI points to the root of the repository.

### Run server
```terminal
dotnet run --project src\TcpServerProgram
```

### Run client
```terminal
dotnet run --project src\TcpClientProgram
```
