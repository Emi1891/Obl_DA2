import { Directive, ElementRef, HostListener, Input, inject } from '@angular/core';

@Directive({
  selector: '[appDigitsOnly]',
  standalone: true
})
export class DigitsOnlyDirective {
  private readonly el = inject(ElementRef<HTMLInputElement>);

  @Input() max?: number;

  @HostListener('input')
  onInput(): void {
    const input = this.el.nativeElement;
    let digits = input.value.replace(/[^0-9]/g, '');
    if (this.max != null && digits !== '' && Number(digits) > this.max) {
      digits = String(this.max);
    }
    if (input.value !== digits) {
      input.value = digits;
    }
  }
}
