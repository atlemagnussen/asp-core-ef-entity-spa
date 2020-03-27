import { userIsLoggedIn, userProfile } from "../store";
import auth from "./authentication.js";
import helper from "./helper.js";

class UserProfile {
    constructor() {
        this.init();
    }
    async init() {
        const currentUser = await auth.getUser();
        if (currentUser) {
            console.log(currentUser);
            userIsLoggedIn.set(true);
        } else {
            console.log("not logged in");
        }
        userIsLoggedIn.subscribe(async value => {
            if (value) {
                this.setLoggedInUserProfile();
            } else {
                this.setLoggedOutUserProfile();
            }
        });
        auth.onUserLoaded((u) => this.onuserIsLoggedIn(u));
    }
    
    onuserIsLoggedIn(loggedInUser) {
        console.log("onuserIsLoggedIn");
        if (loggedInUser) {
            userIsLoggedIn.set(true);
            this.setLoggedInUserProfile(loggedInUser);
        }
    }
    async setLoggedInUserProfile(user) {
        if (!user)
            user = await auth.getUser();
        
        if (!user)
            return;
        
        const up = {
            loggedIn: true,
            id_token: user.id_token,
            access_token: user.access_token,
            session_state: user.session_state,
            refresh_token: user.refresh_token,
            token_type: user.token_type,
            expires_at: user.expires_at,
            scope: user.scope,
            initials: "AA"
        }
        const profileKeys = Object.keys(user.profile);
        profileKeys.forEach((key, index) => {
            up[key] = user.profile[key];
        });
        
        userProfile.set(up);
    }
    setLoggedOutUserProfile() {
        userProfile.set({ loggedIn: false, name: "anon", initials: "U" });
    }
}
export default new UserProfile();
