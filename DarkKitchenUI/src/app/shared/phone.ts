export function formatPhoneDigits(digits: string): string {
  let formatted = '';
  for (let i = 0; i < digits.length; i++) {
    if (i === 3 || i === 6) formatted += ' ';
    formatted += digits[i];
  }
  return formatted;
}

export function toLocalPhone(stored: string): string {
  let digits = stored.replace(/\D/g, '');
  if (digits.startsWith('598')) digits = digits.slice(3);
  return formatPhoneDigits(digits.slice(0, 9));
}

export function formatStoredPhone(stored: string): string {
  const digits = stored.replace(/\D/g, '');
  if (!digits) return '';
  return `+${digits.slice(0, 3)} ${formatPhoneDigits(digits.slice(3, 12))}`;
}
