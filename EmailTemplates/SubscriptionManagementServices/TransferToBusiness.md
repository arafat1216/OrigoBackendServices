### Order Type: {{OrderType}}
---
#### User
**Name:**  {{PrivateSubscription.FirstName}} {{PrivateSubscription.LastName}}

**Born:**  {{PrivateSubscription.BirthDate}}

**Address:**  {{PrivateSubscription.Address}}, {{PrivateSubscription.PostalCode}}, {{PrivateSubscription.PostalPlace}}, {{PrivateSubscription.Country}}

**Email:**  {{PrivateSubscription.Email}}

**MobileNumber:** {{MobileNumber}}


#### Product
**Product Name:** {{SubscriptionProductName}}

**DataPackage:** {{DataPackageName}}

**Transfer Date:** {{OrderExecutionDate}}

#### Customer Reference Fields
| Name    | Type    | Value   |
|---------|---------|---------|
| {{CustomerReferenceFields[0].Name}} | {{CustomerReferenceFields[0].Type}} | {{CustomerReferenceFields[0].Value}} |
| {{CustomerReferenceFields[1].Name}} | {{CustomerReferenceFields[1].Type}} | {{CustomerReferenceFields[1].Value}} |
| {{CustomerReferenceFields[2].Name}} | {{CustomerReferenceFields[2].Type}} | {{CustomerReferenceFields[2].Value}} |
| {{CustomerReferenceFields[3].Name}} | {{CustomerReferenceFields[3].Type}} | {{CustomerReferenceFields[3].Value}} |

#### Owner
**Name:**  {{PrivateSubscription.RealOwner.FirstName}} {{PrivateSubscription.RealOwner.LastName}}

**Born:**  {{PrivateSubscription.RealOwner.BirthDate}}

**Address:**  {{PrivateSubscription.RealOwner.Address}}, {{PrivateSubscription.RealOwner.PostalCode}}, {{PrivateSubscription.RealOwner.PostalPlace}}, {{PrivateSubscription.RealOwner.Country}}

**Email:**  {{PrivateSubscription.RealOwner.Email}}

#### Company
**Name:** {{BusinessSubscription.Name}}

**Organization Number:** {{BusinessSubscription.OrganizationNumber}}

**Address:** {{BusinessSubscription.Address}}, {{BusinessSubscription.PostalCode}}, {{BusinessSubscription.PostalPlace}}, {{BusinessSubscription.Country}}

#### Operator
**Operator Name:** {{OperatorName}}

**Operator Account Payer:** {{OperatorAccountPayer}}

**Operator Account Owner:** {{OperatorAccountName}}

**Operator Account Name:** {{OperatorAccountOwner}}

**SimCard Number:** {{SimCardNumber}}

**SIMCard Action:** {{SIMCardAction}}