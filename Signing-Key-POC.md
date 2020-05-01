# Test signing key rollover

## Keys
See the [Discovery document](https://localhost:6001/.well-known/openid-configuration)  
Here you will find a link to [jwks_uri](https://localhost:6001/.well-known/openid-configuration/jwks)  
If the system works there will be one or more keys here, depending on if you have rolled over at least one time  
The current key will be the first

## JWT tokens
You can copy id token and access token from the client main view when sign in or intercept oidc-client in developer tools.  
Both are signed and can be validated  

Use [jwt.io](https://jwt.io/)  
Paste the whole token into the field where it says `paste a token here`

## JWT results
At the time of writing it would always say "Invalid signature" at once. Even though it makes a request to the discovery document

Copy the public key you believed the token was signed with from [jwks_uri](https://localhost:6001/.well-known/openid-configuration/jwks)  
and paste this into the box where is says "Verify signature" and the `Public key or Certificate` input.

Now it should say `Signature Verified` to the left below the token input box. 