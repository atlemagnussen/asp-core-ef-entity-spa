new Oidc.UserManager({response_mode:"query"}).signinRedirectCallback().then(function(user) {
    console.log("callback-oidc.js");
    console.log(user.state);
    if (user.state)
        window.location = user.state;
    else
        window.location = "/";
}).catch(function(e) {
    console.error(e);
});