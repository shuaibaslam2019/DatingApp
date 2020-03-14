import { Photo } from './photo';

export interface User {
  id: number;
  username: string;
  knownAs: string;
  age: number;
  gender: string;
  createdOn: Date;
  lastActive: Date;
  photoUrl: string;
  city: string;
  country: string;
  interests?: string; // Optional properties always come after the required one
  introduction?: string;
  lookingFor?: string;
  photos?: Photo[];
}
