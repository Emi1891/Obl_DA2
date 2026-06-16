export const MONTH_NAMES = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
];

export function oneMonthAgoRange(): { dateFrom: string; dateTo: string } {
  const today = new Date();
  const monthAgo = new Date();
  monthAgo.setMonth(today.getMonth() - 1);
  return {
    dateFrom: monthAgo.toISOString().slice(0, 10),
    dateTo: today.toISOString().slice(0, 10)
  };
}
