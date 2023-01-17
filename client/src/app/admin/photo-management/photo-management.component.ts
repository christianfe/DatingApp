import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
	selector: 'app-photo-management',
	templateUrl: './photo-management.component.html',
	styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
	photos?: Photo[]

	constructor(private adminService: AdminService) { }

	ngOnInit(): void {
		this.loadPhotos()
	}

	loadPhotos() {
		this.adminService.getUnapprovedPhotos().subscribe({
			next: photos => this.photos = photos
		})
	}
	approvePhoto(id: number) {
		this.adminService.approvePhoto(id);
		this.photos = this.photos?.filter(p => p.id !== id);
	}
	rejectPhoto(id: number) {
		this.adminService.rejectPhoto(id);
		this.photos = this.photos?.filter(p => p.id !== id);
	}

}
