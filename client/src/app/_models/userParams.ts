import { User } from "./user";

export class Userparams {
	gender: string;
	minAge = 18;
	maxAge = 99;
	pageNumber = 1;
	pageSize = 3;
	orderBy = "lastActive";

	constructor(user: User) {
		this.gender = user.gender === 'male' ? 'female' : 'male';
	}

}