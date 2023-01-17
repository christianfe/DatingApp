import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Photo } from '../_models/photo';
import { User } from '../_models/user';

@Injectable({
	providedIn: 'root'
})

export class AdminService {
	baseUrl = environment.apiUrl;

	constructor(private http: HttpClient) { }

	getUsersWithRoles() {
		return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
	}

	updateUserRoles(username: string, roles: string[]) {
		return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/'
			+ username + '?roles=' + roles, {});
	}

	getUnapprovedPhotos() {
		return this.http.get<Photo[]>(this.baseUrl + 'admin/photos-to-moderate');
	}

	approvePhoto(id: number) {
		this.http.put(this.baseUrl + 'admin/approve-photo/' + id, {}).subscribe({
			next: _ => _
		});
	}

	rejectPhoto(id: number) {
		this.http.delete(this.baseUrl + 'admin/reject-photo/' + id).subscribe({
			next: _ => _
		});
	}
}