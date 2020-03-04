import { userIsLoggedIn, userProfile } from "../store";
import helper from "./helper.js";

class UserProfile {
    constructor() {
        this.init();
    }
    async init() {
        // const currentUser = auth.getCurrentUser();
        userIsLoggedIn.subscribe(async value => {
            if (value) {
                this.setLoggedInUserProfile();
            } else {
                this.setLoggedOutUserProfile();
            }
        });
        // auth.handleOAuthRedirects(async () => {
        //     userIsLoggedIn.set(true);
        //     this.setLoggedInUserProfile();
        // });
        firebase.auth().onAuthStateChanged((user) => {
            if (user) {
                this.onuserIsLoggedIn(user);
                if (!user.emailVerified) {
                    user.sendEmailVerification().then(() => {
                        console.log("confirmation mail sent");
                    }).catch((err) => {
                        console.error(err);
                    })
                }
            } else {
                this.setLoggedOutUserProfile();
            }
        });
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
            user = firebase.auth().currentUser;
        
        if (!user)
            return;
        // var emailVerified = user.emailVerified;
        // var photoURL = user.photoURL;
        // var isAnonymous = user.isAnonymous;
        // var providerData = user.providerData;
        //const upServer = await this.get();
        //const up = { id: upServer.id, docId: upServer._id, loggedIn: true, email: upServer.data.email, initials: this.getInitials(upServer.data.email) };
        const up = {
            loggedIn: true,
            id: user.uid,
            email: user.email,
            name: user.displayName,
            initials: helper.getInitials(user.email)
        }
        userProfile.set(up);
    }
    setLoggedOutUserProfile() {
        userProfile.set({ loggedIn: false, name: "anon", initials: "U" });
    }

    updateCurrentUser(displayName) {
        const user = firebase.auth().currentUser;
        if (user) {
            return user.updateProfile({
                displayName
            });
        }
    }
}
export default new UserProfile();
