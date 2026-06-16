import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export function emptyOn404<T>() {
  return (source: Observable<T[]>): Observable<T[]> =>
    source.pipe(
      catchError((err: { status?: number }) =>
        err?.status === 404 ? of([] as T[]) : throwError(() => err))
    );
}
