import {JobListDetails} from "@models/job/JobListDetails";

export interface JobQueryItem {
  items: JobListDetails[];
  totalCount: number;
}
