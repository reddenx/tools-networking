# simple centralized UDP logger

# Purpose
udp logger with simple integrations into ms logging system.

# Use
use logging extensions to integrate with ms logging, special case logging can use the tracelogger

# Packaging
dotnet pack -c Release /p:Version=2.0.0  
dotnet nuget push NAME --api-key KEY --source https://api.nuget.org/v3/index.json
