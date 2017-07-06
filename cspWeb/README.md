# CSP self-service app

I have done this app to demonstrate what a self-service app for a Microsoft CSP (Cloud Solutions Provider) partner might look like.

In essence, the process consists of 3 steps:
1. When the user opens the app for the first time, the main button will take her to the login/register screen. At registration time an Azure subscription will be created
2. After registering and coming back to the main page, the main button will go to the page to create a Recovery Services Vault.
3. Finally, the last step is downloading the MARS agent and the vault credentials file (I am still working in this last point)

You can access the app here: http://cspweb-erjosito.azurewebsites.net/