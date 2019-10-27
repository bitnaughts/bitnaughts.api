![image](https://github.com/bitnaughts/bitnaughts.assets/blob/master/images/banner.png)

# BitNaughts API

Interested in setting up the BitNaughts API development environment? You're at the right place!

## Prerequisites

- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/)
- [Visual Studio Function App Development Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs)

## Installation

First, clone this repository and enter it:

```bash
git clone https://github.com/bitnaughts/bitnaughts.api.git
cd bitnaughts.api
```

Then, run the shell script to install the required BitNaughts module(s). Simply double-click the ```init.sh``` script in the file explorer, or via bash:

```bash
./init.sh
```

Lastly, it is recommended to use GitHub Desktop to manage the submodule(s). To allow GitHub Desktop to track them, open the application and go to ```File```, then ```Add Local Repository``` and navigate to each submodule (e.g. ```bitnaughts.db```) and press ```Add Repository```.

You're all set! Load ```function.app.csproj``` in Visual Studio 2019 and mess around!