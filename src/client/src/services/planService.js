import crud from "./firestoreCrud.js";
import { userIsLoggedIn, plansStore } from "../store";
const collName = "plans";
let plans = [];
class PlanService {
    constructor() {
        this.loggedIn = false;
        userIsLoggedIn.subscribe(async value => {
            this.loggedIn = value;
            if (value) {
                plans = await crud.getAll(collName);
                plansStore.set(plans);
            }
        });

    }
    async getAll() {
        if (!this.loggedIn)
            throw new Error("not logged in");
        
        return plans;
    }
    async create(name) {
        if (!this.loggedIn)
            throw new Error("not logged in");
        
        const plan = await crud.createOrUpdate(collName, {name});
        plans.push(plan);
        plansStore.set(plans);
        return plan;
    }
    async get(id) {
        if (!this.loggedIn)
            throw new Error("not logged in");
        // const plan = await crud.get(collName, id);
        const plan = plans.find(p => p._id === id);
        return Object.assign({}, plan); // return copy
    }
    async update(plan) {
        if (!this.loggedIn)
            throw new Error("not logged in");
        await crud.createOrUpdate(collName, plan);
        const planOrg = plans.find(p => p._id === plan._id);
        Object.assign(planOrg, plan); // merge
        return plan;
    }
}

export default new PlanService();