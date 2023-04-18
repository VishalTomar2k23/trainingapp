import { Component, OnInit } from '@angular/core';
import { HubService } from './core/service/hub-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'nobleui-angular';

  constructor(private hubService: HubService) { }
  ngOnInit(): void {
    this.hubService.createConnection();
  }

}
