# PDF Signing CLI

A simple open-source command line tool for signing PDF files.

This project is designed to be easy to use for developers and also easy to understand for anyone who wants to run the `.exe` file.

## What this program does

This application signs a PDF file by using a certificate file (`.pfx`) and creates a new signed PDF as output.

It can be used in your own project, in scripts, or directly as a compiled executable.

## Open Source

This project is open source and can be used, modified, and improved by the community.

## Third-Party Libraries

This project uses the following third-party NuGet packages:

- PDFsharp (v6.2.4)
- Serilog (v4.2.0)

All credits for these libraries go to their respective authors and maintainers.

## Digital Signature Notice

This project requires a valid digital signature certificate (`.pfx`) to sign PDF files.

Each user must use their own certificate when using this project in a real, productive, or commercial environment.

Any certificate included in this repository is for testing purposes only and must not be used in production.

Users are responsible for obtaining their own valid certificate from an appropriate trusted provider.

## Features

- Sign PDF files from the command line
- Save the signed PDF to a custom output path
- Use a `.pfx` certificate file with password
- Add optional signature information such as reason, location, and contact info
- Return clear exit codes for success and error handling
- Create the output directory automatically if it does not exist
- Log important processing steps and errors for troubleshooting

## CLI usage

The program expects the following values:

### Required parameters

- `inputPdf` - path to the PDF file that should be signed
- `outputSignedPdf` - path where the signed PDF should be saved
- `signaturePath` - path to the certificate file (`.pfx`)
- `publicKey` - password for the certificate

### Optional parameters

- `reason` - reason for the signature
- `location` - signing location
- `contactInfo` - contact details of the signer

## Example

Example PowerShell command for using the CLI:

```bash
PS D:\VisualStudio\tulo.SignPdf\publish\tulo.SigningPdf> .\tulo.SigningPdf.exe --inputPathPdf "D:\VisualStudio\tulo.SignPdf\Shared\Examples\ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3.pdf" --outputPathSignedPdf "D:\VisualStudio\tulo.SignPdf\TestResults\manual_test\ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3_signed.pdf" --signaturePath "D:\VisualStudio\tulo.SignPdf\Shared\Certificates\dummyPdfA3Signing.pfx" --publicKey "12345@" --reason "Unit Test Signatur" --location "Deutschland" --contactInfo "test@example.com"

2026.03.27-11:51:07.317 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Serilog.BootstrapLogger) Bootstrap logger initialized
2026.03.27-11:51:07.343 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.HostBuilders.AddAppSettings) AddAppSettings has been initialized successfully.
2026.03.27-11:51:07.345 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.HostBuilders.AddSerilog) Serilog has been initialized successfully.
2026.03.27-11:51:07.360 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.HostBuilders.AddAppSettings) settings file 'appsettings.json' from app data folder 'D:\VisualStudio\tulo.SignPdf\publish\tulo.SigningPdf-appsettings' not loaded...
2026.03.27-11:51:07.457 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) CLI: SIGNPDF:InputPathPdf = D:\VisualStudio\tulo.SignPdf\Shared\Examples\ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3.pdf
2026.03.27-11:51:07.460 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) CLI: SIGNPDF:OutputPathSignedPdf = D:\VisualStudio\tulo.SignPdf\TestResults\manual_test\ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3_signed.pdf
2026.03.27-11:51:07.460 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) CLI: SIGNPDF:SignaturePath = D:\VisualStudio\tulo.SignPdf\Shared\Certificates\dummyPdfA3Signing.pfx
2026.03.27-11:51:07.460 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) CLI: SIGNPDF:PublicKey = <provided>
2026.03.27-11:51:07.461 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) CLI: SIGNPDF:Reason = Unit Test Signatur
2026.03.27-11:51:07.461 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) CLI: SIGNPDF:Location = Deutschland
2026.03.27-11:51:07.461 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) CLI: SIGNPDF:ContactInfo = test@example.com
2026.03.27-11:51:07.462 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) Starting PDF signing.
2026.03.27-11:51:07.462 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) Input PDF: D:\VisualStudio\tulo.SignPdf\Shared\Examples\ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3.pdf
2026.03.27-11:51:07.463 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) Output signed PDF: D:\VisualStudio\tulo.SignPdf\TestResults\manual_test\ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3_signed.pdf
2026.03.27-11:51:07.577 [USERName] [Thread:1] [ProcessID:4764] [IN] (tulo.SigningPdf.Runners.SignedPdfCliRunner) SUCCESS: Signed PDF created at D:\VisualStudio\tulo.SignPdf\TestResults\manual_test\ZF_Extended__Sammelrechnung_3_Bestellungen_generated_pdfa3_signed.pdf
PS D:\VisualStudio\tulo.SignPdf\publish\tulo.SigningPdf>
```

## What to check before running the `.exe`

Before using the executable, make sure that:

- the input PDF file exists
- the certificate file exists
- the certificate password is correct
- the output folder is writable
- the output path is valid

If one of the required values is missing or invalid, the application returns an error code and writes the problem to the log.

## Logging and troubleshooting

This project uses **Serilog** for logging.

That means the application logs important information during execution, for example:

- which CLI values were provided
- whether optional values were empty
- if a required argument is missing
- if the input PDF file cannot be found
- if the certificate file cannot be found
- when the signing process starts
- when signing fails
- when the signed PDF was created successfully
- unexpected processing errors

This makes troubleshooting much easier when something goes wrong.

## Exit codes

The application uses the following exit codes:

- `0` = Success
- `1` = Missing input PDF path
- `2` = Missing output signed PDF path
- `3` = Missing signature path
- `4` = Missing certificate password
- `5` = Input PDF file not found
- `6` = Certificate file not found
- `7` = PDF signing failed
- `8` = Signed PDF was not created
- `10` = General processing failure

## Output

If the process is successful, the application creates a signed PDF file at the output path you provide.

## Test coverage

The project includes a test that checks whether:

- the input PDF exists
- the certificate exists
- the signing process returns success
- the signed PDF file is created
- the output file is not empty

## For developers

The CLI runner reads configuration values, validates required inputs, checks file paths, creates the output directory if needed, starts the signing process, and verifies that the signed PDF was created successfully.

## Notes

- The output folder is created automatically if it does not exist.
- Logging helps identify missing arguments and file path problems.
- This tool is intended for simple and practical PDF signing.

## License

This project is open source.
- Apache License
- Version 2.0, January 2004
