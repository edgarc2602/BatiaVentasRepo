import { Component, OnChanges, Input, SimpleChanges, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'pagina-widget',
    templateUrl: './paginador.widget.html'
})
export class PaginaWidget implements OnChanges {
    @Input() pagina: number = 0;
    @Input() numPaginas: number = 0;
    @Input() rows: number = 0;
    @Output('chgEvent') changeEvent = new EventEmitter<number>();

    constructor() {}

    makePages(num: number) {
        return new Array(num);
    }

    toPrev() {
        this.move(this.pagina - 1);
    }

    toNext() {
        this.move(this.pagina + 1);
    }

    move(p: number) {
        this.changeEvent.emit(p);
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}