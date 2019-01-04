import { Component, OnInit } from '@angular/core';
import { fromEvent, Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'websocket';
  websocket: WebSocket;
  observableMessage: Observable<MessageEvent>;
  message = {
    received: ''
  };

  constructor(private activatedRoute: ActivatedRoute) {
  }

  ngOnInit() {
    this.activatedRoute.queryParams.subscribe(params => {
      this.connect(params['customerSet']);
    });
  }

  connect(customerSet: string) {
    if (!customerSet) {
      return;
    }
    this.websocket = new WebSocket(`${environment.websocketUrl}?customerSet=${customerSet}`);
    this.observableMessage = fromEvent<MessageEvent>(this.websocket, 'message');
    this.observableMessage.subscribe(evnt => {
      console.log('event received', evnt);
      this.message.received += '<br>' + evnt.data;
    });
  }
}
