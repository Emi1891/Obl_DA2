export function extractApiErrorMessage(err: unknown): string | null {
  const body = (err as { error?: unknown })?.error;
  let message: string | undefined;

  if (typeof body === 'string') {
    try {
      message = (JSON.parse(body) as { message?: string }).message ?? body;
    } catch {
      message = body;
    }
  } else if (body && typeof body === 'object') {
    message = (body as { message?: string }).message;
  }

  if (!message) return null;
  return message.replace(/^[^:]+:\s*/, '').trim() || null;
}
