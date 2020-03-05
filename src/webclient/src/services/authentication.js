import Oidc from "oidc-client";
const rootPath = `${window.location.origin}`;
const oicdConfig = {
    authority: "https://localhost:6001",
    client_id: "webclient",
    redirect_uri: `${rootPath}/callback.html`,
    response_type: "code",
    scope:"openid profile bankApi",
    post_logout_redirect_uri : rootPath,
};

class Authentication {
    
    constructor() {
        this.mgr = new Oidc.UserManager(oicdConfig);
    }
    
    async isLoggedIn() {
        const user = await this.mgr.getUser();
        if (user) {
            log("User logged in", user.profile);
            return true;
        }
        else {
            log("User not logged in");
            return false
        }
    }
    login() {
        this.mgr.signinRedirect();
    }
    logout() {
        this.mgr.signoutRedirect();
    }
    async getUser() {
        const user = await this.mgr.getUser();
        return user;
    }
    async getAccessToken() {
        const user = await this.mgr.getUser();
        return user.access_token;
    }
    async loginEmailPass(email, pass) {
        return firebase.auth().signInWithEmailAndPassword(email, pass);
    }
    hasLoggedInUser() {
        return client.auth.isLoggedIn;
    }
    getCurrentUser() {
        return firebase.auth().currentUser;
    }
    logoutCurrentUser() {
        const user = this.getCurrentUser();
        return client.auth.logoutUserWithId(user.id);
    }
    getUserId() {
        return client.auth.user.id;
    }
    addAuthenticationListener(listener) {
        client.auth.addAuthListener(listener);
    }
    removeAuthenticationListener(listener) {
        client.auth.removeAuthListener(listener);
    }
    handleOAuthRedirects(fn) {
        if (client.auth.hasRedirectResult()) {
            client.auth.handleRedirectResult().then(user => {
                // yes its the logged in user
                fn(user);
            });
        }
    }
    getEmail() {
        if (client.auth.authInfo.loggedInProviderName === "oauth2-google") {
            // return client.auth.authInfo.userProfile.data.email;
            return client.auth.user.profile.data.email;
        }
        return "anon";
    }
    getName() {
        if (client.auth.authInfo.loggedInProviderName === "oauth2-google") {
            // return client.auth.authInfo.userProfile.data.email;
            return client.auth.user.profile.data.name;
        }
        return "anon";
    }
    getFirstName() {
        if (client.auth.authInfo.loggedInProviderName === "oauth2-google") {
            // return client.auth.authInfo.userProfile.data.email;
            return client.auth.user.profile.data.first_name;
        }
        return "anon";
    }
    getLastName() {
        if (client.auth.authInfo.loggedInProviderName === "oauth2-google") {
            // return client.auth.authInfo.userProfile.data.email;
            return client.auth.user.profile.data.last_name;
        }
        return "anon";
    }
    getAllUserData() {
        if (client.auth.authInfo.loggedInProviderName === "oauth2-google") {
            // return client.auth.authInfo.userProfile.data.email;
            return client.auth.user.profile.data;
        }
        return "anon";
    }
}
export default new Authentication();
