# Debian Installation under WSL

## Install Debian

Run this in the console:

```powershell
wsl --install debian --web-download
```

After setup:
- remember your username
- remember your password

## Open APT configuration

```bash
sudo nano /etc/apt/apt.config
```

## Update package lists

```bash
sudo apt update
```

## Upgrade the system

```bash
sudo apt dist-upgrade
```

## Check Debian version

```bash
cat /etc/debian_version
```

## Install rsync

```bash
sudo apt install rsync
```


# Access debian console
```bash
wsl -d debian
```