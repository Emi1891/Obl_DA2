export function extractError(err: unknown): string | null {
  const e = err as { error?: { message?: string } | string };
  const stripPrefix = (msg: string) => msg.replace(/^[^:]+:\s*/, '');

  if (typeof e.error === 'string') {
    try {
      const parsed = JSON.parse(e.error) as { message?: string };
      if (parsed?.message) return stripPrefix(parsed.message);
    } catch {
      // not JSON — fall through and treat the string as the message
    }
    return stripPrefix(e.error);
  }

  return e.error?.message ? stripPrefix(e.error.message) : null;
}
