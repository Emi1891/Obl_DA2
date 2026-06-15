export function extractError(err: unknown): string | null {
  const e = err as { error?: { message?: string } | string };
  if (typeof e.error === 'string') return e.error.replace(/^[^:]+:\s*/, '');
  return e.error?.message?.replace(/^[^:]+:\s*/, '') ?? null;
}
