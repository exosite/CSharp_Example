# Overview
Running the program will provision the device, save the authentication token, then use that authentication token to write to the `data_in` resource. Subsequent runs will not provision (since it's already provisioned) and write to `data_in`.

## Requirements
- dotnet - https://dotnet.microsoft.com/download
- System.Json - `dotnet add package System.Json --version 4.6.0`

## Usage
- Place productID in a file named product_id.txt
- Modify deviceID if desired
- run `dotnet run`

