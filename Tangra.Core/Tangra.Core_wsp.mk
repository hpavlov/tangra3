.PHONY: clean All

All:
	@echo ----------Building project:[ TangraCore - Debug ]----------
	@"$(MAKE)" -f "TangraCore.mk"
clean:
	@echo ----------Cleaning project:[ TangraCore - Debug ]----------
	@"$(MAKE)" -f "TangraCore.mk" clean
