import auth from "./authentication.js";

class FirestoreCrud {
    async getAll(colname) {
        const db = firebase.firestore();
        const colRef = db.collection(colname);
        const all = [];
        try {
            const user = auth.getCurrentUser();
            const docRef = colRef.where("ownerId", "==", user.uid);
            const docs = await docRef.get();
            docs.forEach((d) => {
                const doc = d.data();
                doc._id = d.id;
                all.push(doc);
            });
        }
        catch(e) {
            console.log(e);
        }
        return all;
    }
    async get(colname, id) {
        const db = firebase.firestore();
        const docRef = await db.collection(colname).doc(id).get();
        const doc = docRef.data();
        doc._id = id;
        return doc;
    }
    async createOrUpdate(colname, data, id) {
        if (!data)
            throw new Error("No data to set");
        
        const db = firebase.firestore();
        const colRef = db.collection(colname);
        let docRef;
        if (id) {
            docRef = colRef.doc(id);
        }
        else if (data._id) {
            docRef = colRef.doc(data._id);
            Reflect.deleteProperty(data, "_id");
        }
        else {
            docRef = colRef.doc(); //means create new
        }
        
        if (!data.ownerId) {
            const user = auth.getCurrentUser();
            data.ownerId = user.uid;
        }
        const ret = await docRef.set(data);
        data._id = docRef.id;
        return data;
    }
    async delete(colname, id) {
        const db = firebase.firestore();
        return await db.collection(colname).doc(id).delete();
    }
}

export default new FirestoreCrud();
