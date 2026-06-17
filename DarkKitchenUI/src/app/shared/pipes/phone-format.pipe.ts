import { Pipe, PipeTransform } from '@angular/core';
import { formatStoredPhone } from '../phone';

@Pipe({
  name: 'phoneFormat',
  standalone: true
})
export class PhoneFormatPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    return value ? formatStoredPhone(value) : '';
  }
}
