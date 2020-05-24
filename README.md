# AzureAD-Authentication-in-WinForms
Simple example of how to Authenticate to your Azure Active Directory in WinForms

1. I placed all the necessary stuff inside a single class (clsAzureAD) and left the calling methods in the main form.
You will see that you will only need a few lines of codes to execute authentication. 

![IDE](https://user-images.githubusercontent.com/5296677/82749165-e7c2f880-9dd9-11ea-83e2-067f488a0905.png)

2. Signing in will invoke the familiar Microsoft login page. If you have a Microsoft account, then this is the same Sign-in page.

![Sign-in](https://user-images.githubusercontent.com/5296677/82749245-599b4200-9dda-11ea-87af-522ced0a8480.png)

3. Upon authenticating to your account, the access token is retrieved. I have decoded the access token so that the Identity Claims are readable.

![Signed-in](https://user-images.githubusercontent.com/5296677/82749265-923b1b80-9dda-11ea-9a6e-5b4b3178b7fb.png)

4. You can also see in your Azure portal all the sign-in activities of your app.

![Sign-ins](https://user-images.githubusercontent.com/5296677/82749315-e219e280-9dda-11ea-8776-f2c73cf02ca2.png)
