import {ProgrammingResult} from '@models/job/ProgrammingResult';
import {JobEvent} from '@models/job/JobEvent';

export interface JobDetails {
  jobById: JobDetail;
  jobProgrammingResults: ProgrammingResult[];
  jobProgrammingTimes: ProgrammingTimes[];
  jobStatistics: JobStatistics;
  jobEvents: JobEvent[];
}

export interface JobDetail {
  jobName: string;
  jobDescription: string;
  jobChecksum: string;
  settingChecksum: string;
  createdAt: Date;
}

export interface ProgrammingTimes {
  bucket: string;
  avg_program_duration: number;
  avg_verify_duration: number;
  avg_blank_check_duration: number;
  avg_erase_duration: number;
  avg_overhead: number;
}

export interface JobStatistics {
  total_jobs: number;
  total_success: number;
  total_failures: number;
}
