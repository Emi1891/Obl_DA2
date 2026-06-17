import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

export interface Stat {
  label: string;
  value: string;
  icon: string;
  description: string;
  link?: string;
  linkLabel?: string;
}

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './stat-card.component.html',
  styleUrl: './stat-card.component.css'
})
export class StatCardComponent {
  @Input({ required: true }) stat!: Stat;
}
