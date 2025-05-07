export interface JobEvent {
  type: string;
  title: string;
  message: string | null;
  timestamp: string;
}
