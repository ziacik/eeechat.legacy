ifndef TARGET
	TARGET=./bin/Debug
else
	TARGET=./bin/$(TARGET)
endif

MCS=mcs
ifndef (RELEASE)
	MCSFLAGS=-debug --stacktrace
endif
LIBS=-lib:C:/PROGRA~1/MONO-1~1.8/lib\mono/1.0 -lib:C:/PROGRA~1/MONO-1~1.8/lib\mono/gtk-sharp


EEECLIENT_EXE=$(TARGET)/EeeClient.exe
EEECLIENT_PDB=$(TARGET)/EeeClient.exe
EEECLIENT_SRC=About.cs \
	AssemblyInfo.cs \
	HistoryForm.cs \
	LoginForm.cs \
	MainForm.cs \
	MyBalloon.cs \
	Options.cs \
	Receiver.cs \
	Register.cs \
	Security.cs \
	Sender.cs \
	SettingsManager.cs
EEECLIENT_RES=-resource:About.resx,EeeClient.About.resx \
	-resource:afk.ico,EeeClient.afk.ico \
	-resource:App.ico,EeeClient.App.ico \
	-resource:HistoryForm.resx,EeeClient.HistoryForm.resx \
	-resource:licenses.licx,EeeClient.licenses.licx \
	-resource:LoginForm.resx,EeeClient.LoginForm.resx \
	-resource:MainForm.resx,EeeClient.MainForm.resx \
	-resource:MyBalloon.resx,EeeClient.MyBalloon.resx \
	-resource:notify.ico,EeeClient.notify.ico \
	-resource:Options.resx,EeeClient.Options.resx \
	-resource:Register.resx,EeeClient.Register.resx

$(EEECLIENT_EXE): $(EEECLIENT_SRC) # Missing dependency project EeeCommandProcessor ID:{DB273059-FA70-481D-9F29-14692DDEC6C2}? # Missing dependency project EeeData ID:{F41504E2-DCA4-4559-A84E-C393CB951C83}?
	-mkdir -p $(TARGET)
	$(MCS) $(MCSFLAGS) $(LIBS) -r:System.dll -r:System.Data.dll -r:System.Drawing.dll -r:System.Windows.Forms.dll -r:System.Xml.dll -r:System.Design.dll -r:System.Web.dll -r:C1.Common.dll -r:C1.Win.C1Command.dll -r:C1.Win.C1Input.dll -r:BalloonWindow.dll -r:EeeCommandProcessor.dll -r:EeeData.dll -r:System.Web.Services.dll -r:WRM.PropertyTree.dll -target:winexe -out:$(EEECLIENT_EXE) $(EEECLIENT_RES) $(EEECLIENT_SRC)


# common targets

all:	$(EEECLIENT_EXE)

clean:
	-rm -f "$(EEECLIENT_EXE)" 2> /dev/null
	-rm -f "$(EEECLIENT_PDB)" 2> /dev/null


# project names as targets

EeeClient: $(EEECLIENT_EXE)

