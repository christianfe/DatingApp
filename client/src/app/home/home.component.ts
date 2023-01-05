import { Component } from '@angular/core';

@Component({
	selector: 'app-home',
	templateUrl: './home.component.html',
	styleUrls: ['./home.component.css']
})
export class HomeComponent {
	users: any;
	registerMode = false;

	constructor() { }

	ngOnInit(): void {
	}

	registerToggle() {
		this.registerMode = !this.registerMode;
	}

	cancelRegistermMde(event: boolean) {
		this.registerMode = event;
	}
}
