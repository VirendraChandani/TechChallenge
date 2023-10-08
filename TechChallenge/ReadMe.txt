Privalgo Tech Challenge - Payment Exporter

Background
This challenge is to write 2 payment exporters.

Given a list of payments and a specific bank that the payments are to be exported for, you need to create a CSV file.  

This solution has a basic framework, but please make full use of good OOP techniques to extend the solution as you see fit

TDD/Unit tests are strongly encouraged, and a test class library has been provided with XUnit and FluentAssertion packages already loaded

Feel free to make use of packages where relevant, to avoid reinventing the wheel

Data
The TechChallengeHelper will provide you with a list of payments.

Each payment has the following

BeneficiaryFirstName
BeneficiaryLastName
Address
Postcode
Country
Amount

There are 2 banks that require these payments

Bank Of Ireland
JP Morgan

Exporter services have been created ready for you to extend with the required functionality

Selection Criteria
Each bank has its own selection criteria

Bank Of Ireland
EUR payments to Italy
USD payments above 100000 USD

JP Morgan
GBP Payments
USD Payments <= 100000

Validation Criteria
Once the payments have been selected for the required bank using the selection criteria above, each payment needs 
to be validated against its validation rules below

All Banks
Payment amount cannot be 0
Country is mandatory

Bank Of Ireland
BeneficiaryFirstName cannot contain all numbers
BeneficiaryLastName cannot contain all numbers
EUR Payments must be at least 320 EUR

JP Morgan
Postcode is mandatory for all payments
BeneficiaryName cannot exceed 30 characters where BeneficiaryName is $"{BeneficiaryFirstName} {BeneficiaryLastName}" 

Suggested Methods:

ExtractPaymentsAsync - This could take a list of all of the payments and find all payments that match the bank' selection criteria
ValidatePaymentsAsync - This could take a list of payments and the validation dictionary.  The method can return the payments that are valid and add errors to the dictionary
ExportPaymentsAsync - This could take a list of payments that are valid and return the filename of a generated CSV file which will be output to the console

CSV File Formats
First row of the file is to contain the headings as below

JP Morgan
Beneficiary Name is $"{BeneficiaryFirstName} {BeneficiaryLastName}" 
Currency,Amount,BeneficiaryName,Address,Postcode

Bank Of Ireland 
Currency,Amount,BeneficaryFirstName,BeneficaryLastName,Address