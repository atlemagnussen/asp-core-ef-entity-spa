import Oidc from "oidc-client";
const rootPath = `${window.location.origin}`;
const oicdConfig = {
    authority: "https://localhost:6001",
    client_id: "webclient",
    redirect_uri: `${rootPath}/callback.html`,
    response_type: "code",
    scope:"openid profile bankApi offline_access",
    post_logout_redirect_uri: `${rootPath}`,
    automaticSilentRenew: true
};

class Authentication {
    
    constructor() {
        this.mgr = new Oidc.UserManager(oicdConfig);
        this.mgr.events.addAccessTokenExpiring((m) => this.expiring(m));
        this.mgr.events.addAccessTokenExpired((m) => this.expired(m));
        this.mgr.events.addSilentRenewError((m) => this.renewError(m));
        this.mgr.events.addUserSignedOut((m) => this.signedOut(m));
        this.mgr.events.addUserLoaded((m) => this.uoaded(m));
        this.mgr.events.addUserUnloaded((m) => this.unloaded(m));
        this.mgr.events.addUserSessionChanged((m) => this.sessionChanged(m));
    }
    sessionChanged(msg) {
        console.log("sessionChanged");;
        console.log(msg);
    }
    loaded(msg) {
        console.log("loaded");;
        console.log(msg);
    }
    unloaded(msg) {
        console.log("unloaded");;
        console.log(msg);
    }
    signedOut(msg) {
        console.log("signedOut");;
        console.log(msg);
    }
    renewError(msg) {
        console.log("renewError");;
        console.log(msg);
    }
    expiring(msg) {
        console.log("expiring");;
        console.log(msg);
    }
    expired(msg) {
        console.log("expired");;
        console.log(msg);
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
        if (user && user.expired) {
            const res = await this.mgr.startSilentRenew();
            console.log(res);
        }
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
