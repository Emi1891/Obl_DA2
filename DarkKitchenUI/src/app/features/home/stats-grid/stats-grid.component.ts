import { Component, Input } from '@angular/core';
import { StatCardComponent, Stat } from '../stat-card/stat-card.component';

@Component({
  selector: 'app-stats-grid',
  standalone: true,
  imports: [StatCardComponent],
  templateUrl: './stats-grid.component.html',
  styleUrl: './stats-grid.component.css'
})
export class StatsGridComponent {
  @Input() stats: Stat[] = [];
}
