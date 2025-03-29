#!/usr/bin/env python3
import dbus
from dbus.mainloop.glib import DBusGMainLoop
from gi.repository import GLib
import datetime

# Файл для записи уведомлений
LOG_FILE = "notifications.log"

# Запись начальной информации в лог
timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
initial_log_entry = f"[{timestamp}] Script started.\n"

# Записываем в файл
with open(LOG_FILE, "a") as f:
    f.write(initial_log_entry)

def notification_handler(*args, **kwargs):
    lenags = len(args)
    
    # Извлекаем данные уведомления
    app_name = args[0]  # Имя приложения
    summary = args[2]   # Заголовок уведомления
    body = args[3]      # Текст уведомления
    mainbody = args[4]      # Текст уведомления
    timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")

    # Форматируем запись
    log_entry = f"[{timestamp}] AppName - {app_name} Summary - {summary} Body - {body} MainBody - {mainbody}\n"

    # Записываем в файл
    with open(LOG_FILE, "a") as f:
        f.write(log_entry)

# Настройка D-Bus
DBusGMainLoop(set_as_default=True)
bus = dbus.SessionBus()
bus.add_match_string_non_blocking(
    "eavesdrop=true, interface='org.freedesktop.Notifications', member='Notify'"
)
bus.add_message_filter(lambda bus, message: notification_handler(*message.get_args_list()))

# Запуск главного цикла
loop = GLib.MainLoop()
loop.run()
