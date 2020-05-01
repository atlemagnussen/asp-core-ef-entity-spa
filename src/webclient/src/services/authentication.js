import Oidc from "oidc-client";
import helper from "./helper.js";
const rootPath = `${window.location.origin}`;
// config
let oicdConfig = {
    authority: helper.getAuthServerUrl(),
    client_id: "webclient",
    redirect_uri: `${rootPath}/callback.html`,
    response_type: "code",
    scope:"openid profile api.read api.write email",
    post_logout_redirect_uri: `${rootPath}`,
    accessTokenExpiringNotificationTime: 60,
    silentRequestTimeout: 20000,
    automaticSilentRenew: false,
    silent_redirect_uri: `${rootPath}/silent-renew.html`,
    validateSubOnSilentRenew: true,
    revokeAccessTokenOnSignout : true
};

class Authentication {
    
    constructor() {
        this.mgr = new Oidc.UserManager(oicdConfig);
        this.mgr.events.addUserLoaded((m) => this.loaded(m));
        this.mgr.events.addUserUnloaded((m) => this.unloaded(m));
        this.mgr.events.addUserSignedIn((u) => this.signedIn(u));
        this.mgr.events.addUserSignedOut((m) => this.signedOut(m));
        this.mgr.events.addUserSessionChanged((m) => this.sessionChanged(m));
        this.mgr.events.addAccessTokenExpiring(() => this.expiring());
        this.mgr.events.addAccessTokenExpired(() => this.expired());
        this.mgr.events.addSilentRenewError((m) => this.renewError(m));
    }
    // public methods
    onUserLoaded(fn) {
        this.mgr.events.addUserLoaded((u) => fn(u));
    }

    getAuthServer() {
        return oicdConfig.authority;
    }

    loaded(user) {
        console.log("loaded");
        console.log(user);
    }
    unloaded(msg) {
        console.log("unloaded");
        console.log(msg);
    }
    signedIn(msg) {
        console.log("signedOut");
        console.log(msg);
    }
    signedOut(msg) {
        console.log("signedOut");
        console.log(msg);
    }
    sessionChanged(msg) {
        console.log("sessionChanged");
        console.log(msg);
    }
    async expiring() {
        console.log(`expiring in ${oicdConfig.accessTokenExpiringNotificationTime} seconds`);
        if (!oicdConfig.automaticSilentRenew)
            await this.silentRefresh();
    }
    expired() {
        console.log("expired");
    }
    renewError(msg) {
        console.log("renewError");
        console.log(msg);
    }
    logTimeStamped(msg) {
        var ts = helper.getCurrentDateString();
        console.log(`${ts} ${msg}`);
    }
    async isLoggedIn() {
        const user = await this.mgr.getUser();
        if (user) {
            console.log(`User logged in, ${user.profile}`);
            return true;
        }
        else {
            console.log("User not logged in");
            return false;
        }
    }
    login() {
        this.mgr.signinRedirect();
    }
    logout() {
        this.mgr.signoutRedirect();
    }
    async silentRefresh() {
        console.log("silentRefresh:: manual start");
        try {
            const user = await this.mgr.signinSilent();
            console.log("silentRefresh:: renewal successful");
            return user;
        }
        catch (err) {
            console.log("silentRefresh:: Error from signinSilent:", err.message);
        }
    }
    async getUser() {
        const user = await this.mgr.getUser();
        if (user && user.expired) {
            console.log("user token is expired!");
        }
        return user;
    }
    async getAccessToken() {
        const user = await this.mgr.getUser();
        return user.access_token;
    }
}
export default new Authentication();
