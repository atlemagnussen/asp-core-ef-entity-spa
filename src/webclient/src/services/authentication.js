import Oidc from "oidc-client";
const rootPath = `${window.location.origin}`;
const oicdConfig = {
    authority: "https://localhost:6001",
    client_id: "webclient",
    redirect_uri: `${rootPath}/callback.html`,
    response_type: "code",
    scope:"openid profile bankApi",
    post_logout_redirect_uri: `${rootPath}`
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
    hasLoggedInUser() {
        return client.auth.isLoggedIn;
    }
}
export default new Authentication();
