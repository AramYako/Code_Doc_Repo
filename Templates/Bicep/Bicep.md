
[![Python](https://img.shields.io/pypi/pyversions/azure-cli.svg?maxAge=2592000)](https://pypi.python.org/pypi/azure-cli)
[![Build Status](https://dev.azure.com/azure-sdk/public/_apis/build/status/cli/Azure.azure-cli?branchName=dev)](https://dev.azure.com/azure-sdk/public/_build/latest?definitionId=246&branchName=dev)
[![Slack](https://img.shields.io/badge/Slack-azurecli.slack.com-blue.svg)](https://azurecli.slack.com)

A great cloud needs great tools; we're excited to introduce *Azure CLI*, our next generation multi-platform command line experience for Azure.

Take a test run now from Azure Cloud Shell!

[![](https://shell.azure.com/images/launchcloudshell.png "Launch Azure Cloud Shell")](https://shell.azure.com)

## Installation

Please refer to the [install guide](https://docs.microsoft.com/cli/azure/install-azure-cli) for detailed install instructions.

A list of common install issues and their resolutions are available at [install troubleshooting](https://github.com/Azure/azure-cli/blob/dev/doc/install_troubleshooting.md).

### Developer installation (see below)

- [Docker](#docker)
- [Edge Builds](#edge-builds)
- [Developer Setup](#developer-setup)

## Usage

```bash
$ az [ group ] [ subgroup ] [ command ] {parameters}
```

### Get Started

Please refer to the ["get started" guide](https://docs.microsoft.com/cli/azure/get-started-with-az-cli2) for in-depth instructions.

For usage and help content, pass in the `-h` parameter, for example:

```bash
$ az storage -h
$ az vm create -h
```

### Highlights

Here are a few features and concepts that can help you get the most out of the Azure CLI.
