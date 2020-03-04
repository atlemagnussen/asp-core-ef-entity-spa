class Authentication {
    constructor() {
        this.root = `${window.location.origin}/`;
    }
    async loginAnonymous() {
        const anonCred = new AnonymousCredential();
        this.user = await client.auth.loginWithCredential(anonCred);
        return this.user;
    }
    async loginGoogle() {
        const provider = new firebase.auth.GoogleAuthProvider();
        return firebase.auth().signInWithPopup(provider);
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
