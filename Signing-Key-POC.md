# Test signing key rollover

## Keys
New key first
```json
{
    "keys": [
    {
        "kty": "EC",
        "use": "sig",
        "kid": "4a61ea6dacb948e8867cd916a19ae9b8",
        "alg": "ES256",
        "x": "yaoxwu8TxE5wTpzY-8XYfv-ZFkNw0UrBtk6UNrkVZ4Y",
        "y": "gB76VmtvQIPtj2gftyNaVEWCe-BeqQgtRl-kelQvIlo",
        "crv": "P-256"
    },
    {
        "kty": "EC",
        "use": "sig",
        "kid": "1fb408315b35419f981843729c8c626d",
        "alg": "ES256",
        "x": "4yPuERhVJ_Dri6fjZ4Qv7vFvIcqo9P02PpZuZXY26Qc",
        "y": "teB8q99j-AZFWDc3yaD5dYPfIVuCjMR65mooOJUM59g",
        "crv": "P-256"
    }
    ]
}
```

## Old Tokens
```bash
loggedIn: true
id_token: eyJhbGciOiJFUzI1NiIsImtpZCI6IjFmYjQwODMxNWIzNTQxOWY5ODE4NDM3MjljOGM2MjZkIiwidHlwIjoiSldUIn0.eyJuYmYiOjE1ODc4NDk2OTEsImV4cCI6MTU4Nzg0OTk5MSwiaXNzIjoiaHR0cHM6Ly9hc3AtY29yZS1hdXRoLXNlcnZlci5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6IndlYmNsaWVudCIsImlhdCI6MTU4Nzg0OTY5MSwiYXRfaGFzaCI6IjZ1SWxzdmZUZnk1eldJaW5HZXM5NXciLCJzdWIiOiI3Y2YzMGIxOS1kNTVmLTQxNGUtYmRjNC04MGVhOWNmNzFhMTgiLCJhdXRoX3RpbWUiOjE1ODc3OTgxNDQsImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.dXKZ-XD7rr5vk7GHkZdMkVbFlxYPSx_-SZ1xLuV0VagducQyBWtl5oabqnRJCaIpEy6jFOIJRjcmKPXXtIwxWQ
access_token: eyJhbGciOiJFUzI1NiIsImtpZCI6IjFmYjQwODMxNWIzNTQxOWY5ODE4NDM3MjljOGM2MjZkIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE1ODc4NDk2ODcsImV4cCI6MTU4Nzg1MzI4NywiaXNzIjoiaHR0cHM6Ly9hc3AtY29yZS1hdXRoLXNlcnZlci5henVyZXdlYnNpdGVzLm5ldCIsImF1ZCI6ImJhbmtBcGkiLCJjbGllbnRfaWQiOiJ3ZWJjbGllbnQiLCJzdWIiOiI3Y2YzMGIxOS1kNTVmLTQxNGUtYmRjNC04MGVhOWNmNzFhMTgiLCJhdXRoX3RpbWUiOjE1ODc3OTgxNDQsImlkcCI6ImxvY2FsIiwiZW1haWwiOiJhdGxlbWFnbnVzc2VuQGdtYWlsLmNvbSIsInJvbGUiOiJhZG1pbiIsInNjb3BlIjpbIm9wZW5pZCIsImVtYWlsIiwicHJvZmlsZSIsImFwaS5yZWFkIiwiYXBpLndyaXRlIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.kXTashy2FXMbxLO8t21lJZWJZ92RAIFbUj4yQJyg6j8T5rTWVjFSltz1xn6m4rli5AOtECYPRwSWtY_-30cPdg
session_state: pqr_o3KiBtkA38anrEtWfPeCruSWY5ewLPxzeokTzhw.bFMe9MxwB-B8zWMwckAqpQ
refresh_token: SYjKLZFFqWd0xAOed0-FUvDtzFBzW9g38nzYlRESwVI
token_type: Bearer
expires_at: 1587853290
scope: openid profile email api.read api.write offline_access
initials: AA
s_hash: HWc5Bcj92p8Z35A2_oPE8w
sid: rwhStXB7vT0-4vjx7TCXoQ
sub: 7cf30b19-d55f-414e-bdc4-80ea9cf71a18
auth_time: 1587798144
idp: local
amr: pwd
preferred_username: atlemagnussen@gmail.com
name: atlemagnussen@gmail.com
email: atlemagnussen@gmail.com
email_verified: true
role: admin
expires_at: Sun Apr 26 2020 00:21:30 GMT+0200 (Central European Summer Time)
```

## JWT results
Use [jwt.io](https://jwt.io/)

Paste in the access token or the id token, then the key to validate.

Test old key: OK
Test new key: wait
