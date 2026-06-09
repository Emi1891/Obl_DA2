import { Directive, ElementRef, HostListener, Input, inject } from '@angular/core';

/**
 * Keeps an input's value restricted to digits only, stripping any other
 * character (".", ",", letters, etc.) as it is typed, pasted or dropped.
 * When a `max` is provided, the value is also capped to it.
 */
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
