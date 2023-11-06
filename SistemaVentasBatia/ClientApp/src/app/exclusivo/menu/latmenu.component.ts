import { Component, OnInit } from '@angular/core';
declare var bootstrap: any;

@Component({
    selector: 'lat-menu',
    templateUrl: './latmenu.component.html'
})
export class LatMenuComponent implements OnInit {

    constructor() {
    }
    ngOnInit(): void {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });
    }
}